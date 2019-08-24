using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belajar7.CustomTagHelpers
{
    [HtmlTargetElement("aspbutton")]
    public class AspButtonTagHelper : TagHelper
    {
        public string Type { get; set; } = "Submit";
        public string BackgroundColor { get; set; } = "primary";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", $"btn btn-{BackgroundColor}");
            output.Attributes.SetAttribute("type", Type);
        }
    }
}
