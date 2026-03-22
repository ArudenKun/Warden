using Volo.Abp.Data;
using Warden.Core.FastUndoRedo;

namespace Warden.Core;

public static class UndoRedoServiceExtensions
{
    private const string Tracker = nameof(Tracker);

    extension(UndoRedoService service)
    {
        public RegistrationTracker Tracker
        {
            get
            {
                if (service.GetProperty(Tracker) is RegistrationTracker tracker)
                    return tracker;

                tracker = new RegistrationTracker(service);
                service.SetProperty(Tracker, tracker);
                return tracker;
            }
        }
    }
}
