using ShellType.Data;
using ShellType.Quiz;

namespace ShellType.Tests;

public class ErrorScenarioTests
{
    [Fact]
    public void Every_scenario_has_three_distinct_wrong_fixes()
    {
        foreach (var scenario in ErrorScenarios.All)
        {
            Assert.Equal(3, scenario.WrongFixes.Length);
            Assert.DoesNotContain(scenario.Fix, scenario.WrongFixes);
            Assert.Equal(3, scenario.WrongFixes.Distinct().Count());
        }
    }

    [Fact]
    public void Scenarios_are_unique_and_explained()
    {
        Assert.Equal(ErrorScenarios.All.Count, ErrorScenarios.All.Select(s => s.Error).Distinct().Count());
        Assert.All(ErrorScenarios.All, s => Assert.False(string.IsNullOrWhiteSpace(s.Explanation)));
    }
}

public class QuizGeneratorTests
{
    [Theory]
    [InlineData(QuizMode.Mixed)]
    [InlineData(QuizMode.Commands)]
    [InlineData(QuizMode.Errors)]
    public void Creates_the_requested_number_of_valid_questions(QuizMode mode)
    {
        var questions = QuizGenerator.Create(mode, 10, new Random(3));

        Assert.Equal(10, questions.Count);
        Assert.All(questions, q =>
        {
            Assert.Equal(QuizGenerator.OptionCount, q.Options.Count);
            Assert.Equal(q.Options.Count, q.Options.Distinct().Count());
            Assert.InRange(q.AnswerIndex, 0, q.Options.Count - 1);
        });
        Assert.Equal(10, questions.Select(q => q.Prompt).Distinct().Count());
    }

    [Fact]
    public void Mixed_mode_contains_both_kinds()
    {
        var questions = QuizGenerator.Create(QuizMode.Mixed, 10, new Random(1));

        Assert.Equal(5, questions.Count(q => q.Kind == QuestionKind.WhatDoesItDo));
        Assert.Equal(5, questions.Count(q => q.Kind == QuestionKind.FixTheError));
    }

    [Fact]
    public void Command_questions_answer_with_the_commands_description()
    {
        var command = CommandLibrary.Find("chmod +x deploy.sh")!;
        var question = QuizGenerator.FromCommand(command, new Random(5));

        Assert.Equal(command.Description, question.Answer);
        Assert.Equal(command.Text, question.Prompt);
    }

    [Fact]
    public void Error_questions_never_ask_for_more_than_exist()
    {
        var questions = QuizGenerator.Create(QuizMode.Errors, 1000, new Random(2));

        Assert.Equal(ErrorScenarios.All.Count, questions.Count);
    }
}

public class QuizSessionTests
{
    private readonly ManualTimeProvider _clock = new();

    private static QuizQuestion Question(string prompt, int answer = 0) =>
        new(QuestionKind.WhatDoesItDo, prompt, null,
            ["Extract a tarball", "Create a tarball", "List a tarball", "Delete a file"], answer, "why");

    private QuizSession Session(params QuizQuestion[] questions) => new(questions, _clock);

    [Fact]
    public void All_options_are_visible_before_typing()
    {
        var session = Session(Question("tar -xf a.tar"));

        Assert.Equal(4, session.Visible.Count);
    }

    [Fact]
    public void Typing_filters_options()
    {
        var session = Session(Question("tar -xf a.tar"));
        foreach (var c in "extr") { session.Type(c); }

        var only = Assert.Single(session.Visible);
        Assert.Equal(0, only.Index);
        Assert.Equal([0, 1, 2, 3], only.Highlights);
    }

    [Fact]
    public void Backspace_and_delete_word_widen_the_filter_again()
    {
        var session = Session(Question("tar -xf a.tar"));
        foreach (var c in "list a") { session.Type(c); }
        Assert.Single(session.Visible);

        session.DeleteWord();
        Assert.Equal("list ", session.Query);
        session.ClearQuery();
        Assert.Equal(4, session.Visible.Count);
    }

    [Fact]
    public void Move_wraps_around()
    {
        var session = Session(Question("x"));
        session.Move(-1);

        Assert.Equal(3, session.Selected);
        session.Move(1);
        Assert.Equal(0, session.Selected);
    }

    [Fact]
    public void Submitting_with_no_matches_does_nothing()
    {
        var session = Session(Question("x"));
        foreach (var c in "zzz") { session.Type(c); }
        session.Submit();

        Assert.Equal(QuizPhase.Answering, session.Phase);
    }

    [Fact]
    public void Fast_right_answer_gets_full_speed_bonus()
    {
        var session = Session(Question("x"));
        session.Submit();

        Assert.Equal(QuizPhase.Revealed, session.Phase);
        Assert.True(session.LastAnswer!.Correct);
        Assert.Equal(QuizSession.BasePoints + QuizSession.MaxSpeedBonus, session.Score);
    }

    [Fact]
    public void Slow_answer_gets_no_speed_bonus()
    {
        var session = Session(Question("x"));
        _clock.Advance(30);
        session.Submit();

        Assert.Equal(QuizSession.BasePoints, session.Score);
    }

    [Fact]
    public void Streak_builds_a_combo_multiplier_and_a_miss_resets_it()
    {
        var session = Session(Question("a"), Question("b"), Question("c", answer: 1), Question("d"));

        session.Submit(); session.Submit();          // right, x1   -> 200
        session.Submit(); session.Submit();          // right, x1.5 -> 300
        Assert.Equal(2, session.Streak);
        Assert.Equal(2, session.Multiplier);

        session.Submit(); session.Submit();          // wrong (picked option 0)
        Assert.Equal(0, session.Streak);
        Assert.Equal(1, session.Multiplier);

        session.Submit();                             // right again, x1 -> 200
        Assert.Equal(700, session.Score);
        Assert.Equal(2, session.BestStreak);
    }

    [Fact]
    public void Enter_after_last_reveal_finishes()
    {
        var session = Session(Question("a"));
        session.Submit();
        session.Submit();

        Assert.Equal(QuizPhase.Finished, session.Phase);
        Assert.Equal(1, session.CorrectCount);
    }

    [Fact]
    public void Typing_is_ignored_while_revealed()
    {
        var session = Session(Question("a"), Question("b"));
        session.Submit();
        session.Type('x');

        Assert.Equal("", session.Query);
    }

    [Fact]
    public void Next_question_starts_with_an_empty_filter()
    {
        var session = Session(Question("a"), Question("b"));
        session.Type('e');
        session.Submit();
        session.Submit();

        Assert.Equal(1, session.Index);
        Assert.Equal("", session.Query);
        Assert.Equal(4, session.Visible.Count);
    }
}
