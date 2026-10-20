using ShellType.Data;
using ShellType.Models;
using ShellType.Quiz;

namespace ShellType.Tests;

public class ClaudeAnswersTests
{
    private static ClaudeCommand Find(string key) => ClaudeLibrary.Find(key)!;

    [Theory]
    [InlineData("/compact", "/compact")]
    [InlineData("/compact", "  /COMPACT ")]
    [InlineData("/compact", "/compact keep the api notes")]
    [InlineData("/clear", "/reset")]
    [InlineData("/clear", "/new")]
    [InlineData("Ctrl+R", "ctrl + r")]
    [InlineData("Alt+P", "option+p")]
    [InlineData("Ctrl+V", "alt+v")]
    [InlineData("--worktree, -w", "-w")]
    [InlineData("--worktree, -w", "claude --worktree")]
    [InlineData("--model", "claude --model opus")]
    [InlineData("claude -c", "claude -c")]
    [InlineData("claude -c", "claude --continue")]
    [InlineData("claude -p \"query\"", "claude -p \"summarize the readme\"")]
    [InlineData("claude -p \"query\"", "claude --print hello")]
    [InlineData("claude attach", "claude attach 4f2a")]
    [InlineData("acceptEdits", "acceptedits")]
    [InlineData("Context window", "context window")]
    public void Accepts_right_answers(string key, string typed)
    {
        Assert.True(ClaudeAnswers.IsCorrect(typed, Find(key)), $"{typed} should count for {key}");
    }

    [Theory]
    [InlineData("/compact", "/context")]
    [InlineData("/compact", "")]
    [InlineData("/clear", "/clearall")]
    [InlineData("claude \"query\"", "claude -c")]
    [InlineData("claude -c", "claude -r")]
    [InlineData("--model", "--effort")]
    [InlineData("Ctrl+R", "ctrl+t")]
    public void Rejects_wrong_answers(string key, string typed)
    {
        Assert.False(ClaudeAnswers.IsCorrect(typed, Find(key)), $"{typed} should not count for {key}");
    }
}

public class ClaudeQuizTests
{
    [Fact]
    public void Claude_mode_asks_both_ways_round()
    {
        var questions = QuizGenerator.Create(QuizMode.Claude, 20, new Random(5));

        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhatDoesItDo));
        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhichCommand));
        Assert.All(questions, q =>
        {
            var command = q.Kind == QuestionKind.WhatDoesItDo
                ? ClaudeLibrary.All.Single(c => c.Name == q.Prompt)
                : ClaudeLibrary.All.Single(c => c.Description == q.Prompt);
            Assert.Equal(q.Kind == QuestionKind.WhatDoesItDo ? command.Description : command.Key, q.Answer);
            Assert.Equal(command.Prompt, q.Shell);
            Assert.Equal(ClaudeKinds.Label(command.Kind), q.Topic);
        });
    }

    [Fact]
    public void Distractors_come_from_the_same_kind_first()
    {
        var shortcut = ClaudeLibrary.Find("Ctrl+R")!;
        var question = QuizGenerator.WhichClaude(shortcut, new Random(2));

        Assert.All(question.Options, o => Assert.Equal(ClaudeKind.Shortcut, ClaudeLibrary.Find(o)!.Kind));
    }

    [Fact]
    public void Scenario_mode_answers_with_the_scenario_command()
    {
        var questions = QuizGenerator.Create(QuizMode.ClaudeScenarios, 24, new Random(7));

        Assert.Equal(24, questions.Count);
        Assert.All(questions, q =>
        {
            Assert.Equal(QuestionKind.Scenario, q.Kind);
            Assert.Equal(ClaudeLibrary.Scenarios.Single(s => s.Situation == q.Prompt).Answer, q.Answer);
            Assert.Equal(QuizGenerator.OptionCount, q.Options.Distinct().Count());
        });
    }

    [Fact]
    public void Typed_mode_is_answered_by_typing()
    {
        var questions = QuizGenerator.Create(QuizMode.ClaudeTyped, 10, new Random(1));

        Assert.All(questions, q =>
        {
            Assert.Equal(QuestionKind.TypeIt, q.Kind);
            Assert.True(q.IsTyped);
            Assert.Single(q.Options);
        });
    }

    [Fact]
    public void A_typed_answer_is_checked_on_enter()
    {
        var right = QuizGenerator.TypeClaude(ClaudeLibrary.Find("/clear")!);
        var session = new QuizSession([right, right]);

        session.Submit();
        Assert.Equal(QuizPhase.Answering, session.Phase); // nothing typed yet

        foreach (var c in "/reset")
        {
            session.Type(c);
        }

        session.Submit();
        Assert.True(session.LastAnswer!.Correct);
        Assert.Equal("/reset", session.LastAnswer.Typed);

        session.Submit();
        foreach (var c in "/compact")
        {
            session.Type(c);
        }

        session.Submit();
        Assert.False(session.LastAnswer!.Correct);
        Assert.Equal(-1, session.LastAnswer.ChosenIndex);
    }
}

public class TimeAttackTests
{
    private static QuizQuestion Question(string prompt) =>
        new(QuestionKind.WhatDoesItDo, prompt, null, ["a", "b", "c", "d"], 0, "because");

    [Fact]
    public void Ends_when_the_time_is_up_and_counts_only_answered_questions()
    {
        var clock = new ManualTimeProvider();
        var session = new QuizSession([Question("a"), Question("b"), Question("c")], clock, TimeSpan.FromSeconds(60));

        session.Submit();
        session.Submit();
        clock.Advance(30);
        session.Tick();
        Assert.Equal(QuizPhase.Answering, session.Phase);
        Assert.Equal(30, session.Remaining!.Value.TotalSeconds);

        clock.Advance(31);
        session.Tick();
        Assert.Equal(QuizPhase.Finished, session.Phase);
        Assert.Equal(TimeSpan.Zero, session.Remaining);
        Assert.Equal(1, session.Total);
        Assert.Equal(1, session.CorrectCount);
    }

    [Fact]
    public void A_quiz_without_a_limit_has_no_clock()
    {
        var session = new QuizSession([Question("a")]);

        Assert.Null(session.Remaining);
        Assert.Equal(1, session.Total);
    }
}
