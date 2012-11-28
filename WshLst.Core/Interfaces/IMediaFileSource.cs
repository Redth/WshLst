using System.Threading.Tasks;
using Xamarin.Media;

namespace WshLst.Core.Interfaces
{
    public interface IMediaFileSource
    {
        Task<MediaFile> GetPhoto(bool takeNew);
    }
}