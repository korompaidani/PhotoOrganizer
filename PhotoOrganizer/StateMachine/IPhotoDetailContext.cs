using PhotoOrganizer.UI.StateMachine.MetaSerializationStates;

namespace PhotoOrganizer.UI.StateMachine
{
    public interface IPhotoDetailContext
    {
        void RunWorkflow(IPhotoDetailState initialState);
        void TransitionTo(IPhotoDetailState state);
    }
}
