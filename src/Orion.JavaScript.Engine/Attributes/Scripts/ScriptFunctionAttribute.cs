namespace Orion.JavaScript.Engine.Attributes.Scripts;

[AttributeUsage(AttributeTargets.Method)]
public class ScriptFunctionAttribute : Attribute
{
    public string? HelpText { get; }

    public ScriptFunctionAttribute(string? helpText = null)
    {
        HelpText = helpText;
    }
}
