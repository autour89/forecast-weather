using AutoMapper;
using Forecast.Core.Models.DAOs;
using Forecast.Core.Models.DTOs;

namespace Forecast.Core.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WeatherResponse, WeatherData>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Sys!.Country))
            .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Main!.Temp))
            .ForMember(dest => dest.FeelsLike, opt => opt.MapFrom(src => src.Main!.FeelsLike))
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => GetFirstWeather(src)!.Description)
            )
            .ForMember(
                dest => dest.MainWeather,
                opt => opt.MapFrom(src => GetFirstWeather(src)!.Main)
            )
            .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Main!.Humidity))
            .ForMember(dest => dest.WindSpeed, opt => opt.MapFrom(src => src.Wind!.Speed))
            .ForMember(dest => dest.Pressure, opt => opt.MapFrom(src => src.Main!.Pressure))
            .ForMember(dest => dest.IconCode, opt => opt.MapFrom(src => GetFirstWeather(src)!.Icon))
            .ForMember(
                dest => dest.LastUpdated,
                opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Dt).DateTime)
            )
            .ForMember(dest => dest.UseCelsius, opt => opt.Ignore());
    }

    private static Weather? GetFirstWeather(WeatherResponse response) =>
        response.Weather?.FirstOrDefault();
}
