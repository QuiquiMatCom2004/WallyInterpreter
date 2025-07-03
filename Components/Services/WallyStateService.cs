using WallyInterpreter.Components.Draw;

namespace WallyInterpreter.Components.Services
{
    public class WallyStateService
    {
        public Colors[,]? Matrix { get; private set; }

        public event EventHandler? OnMatrixUpdated;

        public void SetMatrix(Colors[,] mat)
        {
            Matrix = mat;
            OnMatrixUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
