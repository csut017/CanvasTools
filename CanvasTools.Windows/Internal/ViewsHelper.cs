using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace CanvasTools.Windows.Internal;

internal static class ViewsHelper
{
    public static void RegisterViews<TType>(ResourceDictionary resources, ServiceCollection serviceCollection)
    {
        var baseNamespaceLength = typeof(TType).Namespace?.Length + 1 ?? 0;
        var allTypes = typeof(TType).Assembly
            .GetTypes()
            .Where(t => t.FullName != null && t.FullName.Length > baseNamespaceLength)
            .ToDictionary(t => t.FullName![baseNamespaceLength..]);
        var viewMappings = new ResourceDictionary();
        foreach (var viewModel in allTypes.Where(t => t.Key.StartsWith("ViewModels.")))
        {
            var viewName = viewModel.Key.Replace("ViewModels.", "Views.");
            if (viewName == viewModel.Key
                || !allTypes.TryGetValue(viewName, out var viewType)
                || !(viewType.IsAssignableTo(typeof(UserControl)) || viewType.IsAssignableTo(typeof(Window)))) continue;
            var template = CreateTemplate(viewModel.Value, viewType);
            if (template.DataTemplateKey == null) continue;
            viewMappings.Add(template.DataTemplateKey, template);
            serviceCollection.AddTransient(viewModel.Value);
        }

        resources.MergedDictionaries.Add(viewMappings);
    }

    private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
    {
        var xaml = $"<DataTemplate DataType=\"{{x:Type vm:{viewModelType.Name}}}\"><v:{viewType.Name} /></DataTemplate>";
        var context = new ParserContext
        {
            XamlTypeMapper = new XamlTypeMapper([])
        };
        context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
        context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);
        context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        context.XmlnsDictionary.Add("vm", "vm");
        context.XmlnsDictionary.Add("v", "v");

        var template = (DataTemplate)XamlReader.Parse(xaml, context);
        return template;
    }
}