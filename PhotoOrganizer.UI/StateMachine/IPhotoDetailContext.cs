using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.StateMachine.MetaSerializationStates;

namespace PhotoOrganizer.UI.StateMachine
{
    public interface IPhotoDetailContext
    {
        void RunWorkflow(IPhotoDetailState initialState, PhotoDetailInfo photoDetailInfo);
        void TransitionTo(IPhotoDetailState state, PhotoDetailInfo photoDetailInfo);
    }
}
