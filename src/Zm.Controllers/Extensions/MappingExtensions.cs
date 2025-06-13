using AutoMapper;
using Zm.Logging.Static;

namespace Zm.Controllers.Extensions;

public static class MappingExtensions
{
    public static TDestination? SafeMap<TSource, TDestination>(this IMapper mapper, TSource source)
    {
        if (source == null)
        {
            Logger.LogWarning(
                $"Mapping warning: Source object is null (Type: {typeof(TSource).Name} -> {typeof(TDestination).Name})");
            return default;
        }

        try
        {
            return mapper.Map<TDestination>(source);
        }
        catch (Exception ex) when (ex is AutoMapperMappingException or FormatException or ArgumentException)
        {
            Logger.LogError($"{ex.GetType().Name}: {ex.Message}");
            return default;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Unexpected mapping error: {ex.Message}");
            return default;
        }
    }

    public static IEnumerable<TDestination>? SafeMapList<TSource, TDestination>(this IMapper mapper,
        IEnumerable<TSource> source)
    {
        try
        {
            return mapper.Map<IEnumerable<TDestination>>(source);
        }
        catch (Exception ex) when (ex is AutoMapperMappingException || ex is FormatException || ex is ArgumentException)
        {
            Logger.LogError($"{ex.GetType().Name}: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Unexpected mapping error: {ex.Message}");
            return null;
        }
    }
}