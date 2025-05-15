namespace Orion.JavaScript.Engine.Attributes.Scripts;

[AttributeUsage(AttributeTargets.Class)]
public class ScriptModuleAttribute : Attribute
{
    public string Name { get; }

    public ScriptModuleAttribute(string name)
    {
        Name = name;
    }
}
