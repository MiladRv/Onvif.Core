using Onvif.Core.Client.Common;
using Onvif.Core.Client.Imaging;
using Onvif.Core.Client.Media;
using Onvif.Core.Client.Ptz;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Onvif.Core.Client.Camera
{
    public class Camera
    {
        public async Task<Camera> Create(Account account,
            Action<Exception> exception,
            CancellationToken cancellationToken = default)
        {
            var camera = new Camera(account);

            var usable = await Testing(exception, cancellationToken);

            return usable ? camera : null;
        }

        public AutoFocusMode FocusMode { get; set; }

        public System.DateTime LastUsed { get; set; }
        
        private Account Account { get; }
        public Camera(Account account)
        {
            Account = account;
        }

        private async Task<bool> Testing(Account account, Action<Exception> exception)
        {
            try
            {
                //var device = await OnvifClientFactory.CreateDeviceClientAsync(account.Host, account.UserName, account.Password);
                var response = await Media.GetProfilesAsync();
                return true;
            }
            catch (Exception ex)
            {
                exception?.Invoke(ex);
                return false;
            }
        }

        private async Task<bool> Testing(Action<Exception> exception, CancellationToken cancellationToken = default)
        {
            try
            {
                //var device = await OnvifClientFactory.CreateDeviceClientAsync(account.Host, account.UserName, account.Password);

                var task = await Task.Run(async () =>
                    await Media.GetProfilesAsync(), cancellationToken: cancellationToken);

                var profiles = task.Profiles;

                return true;
            }
            catch (Exception ex)
            {
                exception?.Invoke(ex);
                return false;
            }
        }


        private PTZClient ptz;
        public PTZClient Ptz
        {
            get
            {
                ptz ??= OnvifClientFactory.CreatePTZClientAsync(Account.Host, Account.UserName, Account.Password).Result;
                return ptz;
            }
        }


        private MediaClient media;
        public MediaClient Media
        {
            get
            {
                media ??= OnvifClientFactory.CreateMediaClientAsync(Account.Host, Account.UserName, Account.Password).Result;
                return media;
            }
        }


        private ImagingClient imaging;
        public ImagingClient Imaging
        {
            get
            {
                imaging ??= OnvifClientFactory.CreateImagingClientAsync(Account.Host, Account.UserName, Account.Password).Result;
                return imaging;
            }
        }

        private Profile profile;
        public Profile Profile
        {
            get
            {
                if (profile != null)
                    return profile;

                var response = Media.GetProfilesAsync().Result;
                profile = response.Profiles[0];
                return profile;
            }
        }

        private VideoSource videoSource;
        public VideoSource VideoSource
        {
            get
            {
                if (videoSource != null)
                    return videoSource;

                var response = Media.GetVideoSourcesAsync().Result;
                videoSource = response.VideoSources[0];
                return videoSource;
            }
        }

    }
}
