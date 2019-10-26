using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("WPFCore")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ICEP GmbH")]
[assembly: AssemblyProduct("WPFCore")]
[assembly: AssemblyCopyright("Copyright © ICEP GmbH 2015, 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("a39050ee-8ae9-4d3f-b0ba-1d0aec0e42b8")]

[assembly: ThemeInfo(
    // Specifies the location of system theme-specific resource dictionaries for this project.
    // The default setting in this project is "None" since this default project does not
    // include these user-defined theme files:
    //     Themes\Aero.NormalColor.xaml
    //     Themes\Classic.xaml
    //     Themes\Luna.Homestead.xaml
    //     Themes\Luna.Metallic.xaml
    //     Themes\Luna.NormalColor.xaml
    //     Themes\Royale.NormalColor.xaml
    ResourceDictionaryLocation.SourceAssembly,

    // Specifies the location of the system non-theme specific resource dictionary:
    //     Themes\generic.xaml
    ResourceDictionaryLocation.SourceAssembly)]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: XmlnsPrefix("http://schemas.ICEP.com/wpf/2015/xaml", "wpc")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.Data")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.XAML")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.XAML.Controls")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.XAML.Behaviors")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.XAML.Converter")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.XAML.DragDrop")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.StatusText")]
[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.DataKiosk")]


[assembly: XmlnsDefinition("http://schemas.ICEP.com/wpf/2015/xaml", "WPFCore.UserAttraction")]
