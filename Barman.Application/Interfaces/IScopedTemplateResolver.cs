namespace Barman.Application.Interfaces;

public interface IScopedTemplateResolver
{
    bool IsMatch(
        Guid? templateValue,
        Guid? contextValue);

    int Specificity(
        Guid? templateValue,
        Guid? contextValue);
}