using Autofac;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class ClosingPhotoDetailState : PhotoDetailState
    {
        public override void Handle()
        {
            var newState = Bootstrapper.Container.Resolve<ClosedPhotoDetailState>();
            _context.TransitionTo(newState);
            newState.Handle();
        }
    }
}
