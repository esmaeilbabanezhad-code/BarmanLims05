using Barman.Application.Interfaces;

namespace Barman.Application.Services.Resolvers;

public class ScopedTemplateResolver : IScopedTemplateResolver
{
    public bool IsMatch(
        Guid? templateValue,
        Guid? contextValue)
    {
        return templateValue == null ||
               templateValue == contextValue;
    }

    public int Specificity(
        Guid? templateValue,
        Guid? contextValue)
    {
        return templateValue.HasValue &&
               contextValue.HasValue &&
               templateValue == contextValue
            ? 1
            : 0;
    }
}