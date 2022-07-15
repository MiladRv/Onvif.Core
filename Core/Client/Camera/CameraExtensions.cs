using Onvif.Core.Client.Common;
using System.Threading.Tasks;

namespace Onvif.Core.Client.Camera
{
    public static class CameraExtensions
    {
        public static async Task<bool> FocusAsync(this Camera camera, FocusMove focusMove)
        {
            if (camera == null)
                return false;

            var imaging = camera.Imaging;
            var focus = camera.VideoSource.Imaging.Focus != null;
            var videoSourceToken = camera.VideoSource.token;

            const AutoFocusMode focusMode = AutoFocusMode.MANUAL;

            if (focus)// && camera.FocusMode != focusMode
            {
                var imageSettings = await imaging.GetImagingSettingsAsync(videoSourceToken);
                if (imageSettings.Focus == null)
                {
                    imageSettings.Focus = new FocusConfiguration20
                    {
                        AutoFocusMode = focusMode
                    };

                    await imaging.SetImagingSettingsAsync(videoSourceToken, imageSettings, false);
                }
                else if (imageSettings.Focus.AutoFocusMode != focusMode)
                {
                    imageSettings.Focus.AutoFocusMode = focusMode;
                    await imaging.SetImagingSettingsAsync(videoSourceToken, imageSettings, false);
                }
                camera.FocusMode = focusMode;
            }

            await imaging.MoveAsync(videoSourceToken, focusMove);
            return true;
        }

        public static async Task<bool> MoveAsync(this Camera camera, MoveType moveType, PTZVector vector, PTZSpeed speed, int timeout)
        {
            if (camera == null) 
                return false;

            var ptz = camera.Ptz;

            var profileToken = camera.Profile.token;
              
            switch (moveType)
            {
                case MoveType.Absolute:
                    await ptz.AbsoluteMoveAsync(profileToken, vector, speed);
                    return true;
                case MoveType.Relative:
                    await ptz.RelativeMoveAsync(profileToken, vector, speed);
                    return true;
                case MoveType.Continuous:
                    await ptz.ContinuousMoveAsync(profileToken, speed, timeout.ToString());
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}
