using System.Collections.ObjectModel;
using CanvasTools.Connection;
using CanvasTools.Windows.Interfaces;

namespace CanvasTools.Windows.ViewModels;

public class CourseList(IShell shell, ICanvas canvas)
    : IInitialisable
{
    /// <summary>
    /// The courses for this view model.
    /// </summary>
    public ObservableCollection<ICourse> Courses { get; } = [];

    /// <summary>
    /// Initialise the view model.
    /// </summary>
    public void Initialise()
    {
        shell.UpdateStatus("Initialising course list...", true);
        List<ICourse> courses = [];
        Task.Run(async () =>
            {
                shell.UpdateStatus("Retrieving courses from Canvas...", true);
                courses = await canvas.ListCoursesForCurrentUser().ToListAsync();
            })
            .ContinueWith(t =>
            {
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
                shell.UpdateStatus($"Loaded {courses.Count} course(s) from Canvas");
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}