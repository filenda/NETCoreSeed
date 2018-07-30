using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreSeed.Web.Profiles
{
    public class NETCoreSeedProfile: Profile
    {
        CreateMap<ActivityDTO, ActivityViewModel>().ReverseMap();
        CreateMap<GymDTO, GymViewModel>()
                .ForMember(g => g.PlanName, opts => opts.MapFrom(g => g.Plan.PlanName))
                .ForMember(g => g.CoverPicture, opts => opts.Ignore())
                .ForMember(g => g.BackgroundPicture, opts => opts.Ignore())
                .ForMember(g => g.GalleryPictures, opts => opts.Ignore())
                .ForMember(g => g.CoverPictureArray, opts => opts.MapFrom(g => g.CoverPicture))
                .ForMember(g => g.BackgroundPictureArray, opts => opts.MapFrom(g => g.BackgroundPicture))
                .ForMember(g => g.GalleryPicturesArray, opts => opts.MapFrom(g => g.Pictures))
                .ReverseMap()
                .ForMember(g => g.CoverPicture, opts => opts.ResolveUsing(g =>
                {
            if (g.CoverPicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    g.CoverPicture.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }))
                .ForMember(g => g.BackgroundPicture, opts => opts.ResolveUsing(g =>
                {
            if (g.BackgroundPicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    g.BackgroundPicture.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }))
                .ForMember(g => g.Pictures, opts => opts.ResolveUsing(g =>
                {
            if (g.GalleryPictures != null)
            {
                List<byte[]> pics = new List<byte[]>();
                foreach (var pic in g.GalleryPictures)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        pic.CopyTo(memoryStream);
                        pics.Add(memoryStream.ToArray());
                    }
                }
                return pics.ToArray();
            }
            return null;
        }));
    }
}
