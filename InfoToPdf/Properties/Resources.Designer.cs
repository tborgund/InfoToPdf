﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InfoToPdf.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("InfoToPdf.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE HTML PUBLIC &quot;-//W3C//DTD XHTML 1.0 Transitional//EN&quot; &quot;http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd&quot;&gt;
        ///&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;style&gt;
        ///    body {
        ///        color: #333;
        ///        font: 16px/1.5em &quot;HelveticaNeue-Light&quot;, &quot;Helvetica Neue Light&quot;, &quot;Helvetica Neue&quot;, Helvetica, Arial, &quot;Lucida Grande&quot;, sans-serif;
        ///        font-weight: normal;
        ///        margin: 0;
        ///        padding: 0;
        ///    }
        ///
        ///    #container {
        ///        clear: both;
        ///        background: #fff;
        ///        margin:  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htmlStart {
            get {
                return ResourceManager.GetString("htmlStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html PUBLIC &quot;-//W3C//DTD XHTML 1.0 Transitional//EN&quot; &quot;http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd&quot;&gt;
        ///&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///	&lt;style&gt;
        ///
        ///		body { 
        ///			color:#333;
        ///			font: 17px/1.5em &quot;HelveticaNeue-Light&quot;, &quot;Helvetica Neue Light&quot;, &quot;Helvetica Neue&quot;, Helvetica, Arial, &quot;Lucida Grande&quot;, sans-serif;
        ///			font-weight: normal;
        ///			margin:0;
        ///			padding:0;
        ///
        ///		}
        ///		
        ///		#container {
        ///			clear: both;
        ///			
        ///			background:#fff;
        ///
        ///			margin:0 auto;
        ///			padding-top:10px [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htmlStartPlain {
            get {
                return ResourceManager.GetString("htmlStartPlain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;no&quot;&gt;&lt;head&gt;&lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=UTF-8&quot;&gt;
        ///	&lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta http-equiv=&quot;CACHE-CONTROL&quot; content=&quot;NO-CACHE&quot;&gt;
        ///	&lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1, maximum-scale=1&quot;&gt;
        ///	&lt;title&gt;Konto informasjon&lt;/title&gt;
        ///
        ///	&lt;style&gt;
        ///
        ///        .overlay {
        ///            top:0; left:0;
        ///            background:#7f7f7f;
        ///            opacity: 0.4;
        ///            filter: alpha(opacity=40);
        ///            width: 100%;
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string kontoinfo {
            get {
                return ResourceManager.GetString("kontoinfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] wkhtmltopdf {
            get {
                object obj = ResourceManager.GetObject("wkhtmltopdf", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}