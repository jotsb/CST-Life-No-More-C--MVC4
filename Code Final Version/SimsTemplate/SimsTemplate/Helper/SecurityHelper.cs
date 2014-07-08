using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace SimsTemplate.Helper
{

    /// <summary>
    /// @Author: Aubrey & Patrick
    /// Static helper class to strip off HTML or JS tags from user input.
    /// Security Helper.
    /// </summary>
    public static class SecurityHelper
    {

        /// <summary>
        /// RegEx for tags : html or JS.
        /// </summary>
        const string HTML_TAG_PATTERN = "<.*?>";

        /// <summary>
        /// RegEx for tags : html or JS excluding anchor elements.
        /// </summary>
        const string HTML_TAG_PATTERN_EXCLUDING_LINKS = "<.*?>";

        /// <summary>
        /// @Author: Aubrey
        /// To strip the html or script tags off of strings
        /// </summary>
        /// <param name="inputString">The original string.</param>
        /// <returns>The tag - free original string.</returns>
        public static string StripHTML(string inputString)
        {
            return Regex.Replace
              (inputString, HTML_TAG_PATTERN, string.Empty);
        }

        /// <summary>
        /// @Author: Patrick
        /// To strip the html or script tags off of strings except for anchor elements
        /// </summary>
        /// <param name="inputString">The original string.</param>
        /// <returns>The tag - free original string.</returns>
        public static string StripHTMLExceptAnchor(string inputString)
        {
            return Regex.Replace
              (inputString, HTML_TAG_PATTERN_EXCLUDING_LINKS, string.Empty);
        }
    }
}