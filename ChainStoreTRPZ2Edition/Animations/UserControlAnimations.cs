using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ChainStoreTRPZ2Edition.Animations
{
    public static class UserControlAnimations
    {
        public const float FadeTimeDefaultValue = 1f;

        public static async Task FadeIn(this UserControl userControl, float seconds = FadeTimeDefaultValue)
        {
            var sb = new Storyboard();
            sb.AddFadeIn(seconds);
            sb.Begin(userControl);
            await Task.Delay((int) (seconds * 1000));
        }

        public static async Task FadeOut(this UserControl userControl, float seconds = FadeTimeDefaultValue)
        {
            var sb = new Storyboard();
            sb.AddFadeOut(seconds);
            sb.Begin(userControl);
            await Task.Delay((int) (seconds * 1000));
        }
    }
}