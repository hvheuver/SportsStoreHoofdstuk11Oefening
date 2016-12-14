using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.Domain
{
    public enum Availability
    {
        [Display(Name="Shop only")]
        ShopOnly,
        [Display(Name = "Online only")]
        OnlineOnly,
        [Display(Name = "Shop And Online")]
        ShopAndOnline
    }
}