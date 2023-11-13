using Bunit;
using JetBrains.Annotations;
using KHtmx.Components.Utility;
using KHtmx.Components.Utility.Data;

namespace KHtmx.Tests.Components.Utility;

[UsesVerify]
[TestSubject(typeof(LabelledInput))]
public sealed class LabelledInputTests
{
    private static readonly string[] IgnoreMembers =
    [
        "IsDisposed",
        "ComponentId",
        "Nodes",
        "RenderCount",
        "Services",
    ];

    [Fact]
    public Task ShouldBeAbleToRender()
    {
        using var context = new TestContext();

        var component = context.RenderComponent<LabelledInput>([
            ComponentParameter.CreateParameter("Id", "labelled-input"),
            ComponentParameter.CreateParameter("Label", "Label")
        ]);

        return Verify(component)
            .IgnoreMembers(IgnoreMembers);
    }

    [Fact]
    public Task ShouldBeInlineLabel_WhenLabelTypeIsInline()
    {
        using var context = new TestContext();

        var component = context.RenderComponent<LabelledInput>([
            ComponentParameter.CreateParameter("LabelType", LabelType.Inline),
            ComponentParameter.CreateParameter("Id", "labelled-input"),
            ComponentParameter.CreateParameter("Label", "Label")
        ]);

        return Verify(component)
            .IgnoreMembers(IgnoreMembers);
    }

    [Fact]
    public Task ShouldBeFloatingLabel_WhenLabelTypeIsFloating()
    {
        using var context = new TestContext();

        var component = context.RenderComponent<LabelledInput>([
            ComponentParameter.CreateParameter("LabelType", LabelType.Floating),
            ComponentParameter.CreateParameter("Id", "labelled-input"),
            ComponentParameter.CreateParameter("Label", "Label")
        ]);

        return Verify(component)
            .IgnoreMembers(IgnoreMembers);
    }
}
