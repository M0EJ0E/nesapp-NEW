using System.Threading.Tasks;

namespace nesapp
{
    public class GameService
    {
        private readonly Task _threatCooldownTask;
        private readonly Task _perkGenerationTask;

        public GameService()
        {
            _threatCooldownTask = ThreatCooldownTask;
            _perkGenerationTask = PerkGenerationTask;
        }

        public const long PaperclipBoxCost = 20;
        public const float PaperclipBoxBaseRatePerSecond = 1.0f;
        public const float ThreatLevelCooldownPerSecond = 0.97f;
        public const float ThreatLevelScalingPerSteal = 1.02f;

        public long PaperclipCount { get; private set; }

        public long PaperclipBoxCount { get; private set; }

        public float ThreatLevel { get; private set; } //who knows...

        public delegate void ShouldRenderHandler();

        public event ShouldRenderHandler ShouldRender;

        public void StealPaperclip()
        {
            PaperclipCount++;
            ThreatLevel += 1;
            ThreatLevel = ThreatLevel * ThreatLevelScalingPerSteal;
            ShouldRender?.Invoke();
        }

        public bool CanBuyPaperclipBox => PaperclipCount > PaperclipBoxCost;

        public void BuyPaperclipBox()
        {
            PaperclipCount -= PaperclipBoxCost;
            PaperclipBoxCount++;
            ShouldRender?.Invoke();
        }

        private Task ThreatCooldownTask => Task.Run(async () =>
        {
            while (true)
            {
                if (ThreatLevel > 0)
                {
                    ThreatLevel *= ThreatLevelCooldownPerSecond;
                }

                ShouldRender?.Invoke();
                await Task.Delay(500);
            }
        });

        private Task PerkGenerationTask => Task.Run(async () =>
        {
            while (true)
            {
                PaperclipCount += (long)(PaperclipBoxBaseRatePerSecond * PaperclipBoxCount);
                ShouldRender?.Invoke();
                await Task.Delay(1000);
            }
        });
    }
}
