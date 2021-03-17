using Autofac;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class OpeningPhotoDetailState : PhotoDetailState
    {
        public override void Handle()
        {
            // File operations


            var newState = Bootstrapper.Container.Resolve<OpenPhotoDetailState>();
            _context.TransitionTo(newState);
            newState.Handle();
        }
    }
}
