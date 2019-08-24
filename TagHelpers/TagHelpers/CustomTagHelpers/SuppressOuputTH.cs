using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpers.CustomTagHelpers
{
    [HtmlTargetElement(Attributes = "action-name")]
    public class SuppressOuputTH : TagHelper
    {
        public string ActionName { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!ViewContext.RouteData.Values["action"].ToString().Equals(ActionName))
                output.SuppressOutput();
        }
    }
}
