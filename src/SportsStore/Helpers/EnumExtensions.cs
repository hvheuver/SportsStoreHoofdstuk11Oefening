using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsStore.Helpers
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
          where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values =
                Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Select(e => new {Id = e, Name = e.ToDescription()});
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static string ToDescription<TEnum>(this TEnum e )
        {
            
            DisplayAttribute[] attributes = (DisplayAttribute[])
                e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Name;
                return e.ToString();
        }
    }
}


