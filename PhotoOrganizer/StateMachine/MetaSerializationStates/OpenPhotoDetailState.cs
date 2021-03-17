using Autofac;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class OpenPhotoDetailState : PhotoDetailState
    {
        public override void Handle()
        {
            var newState = Bootstrapper.Container.Resolve<ClosingPhotoDetailState>();
            _context.TransitionTo(newState);
            newState.Handle();
        }
    }
}
