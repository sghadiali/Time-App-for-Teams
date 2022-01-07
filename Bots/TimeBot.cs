// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.0

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TimeApp.Bots
{
    public class TimeBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Echo: {turnContext.Activity.Text}";

            string api = "API_KEY";
            var GoogleLocationService = new GoogleMaps.LocationServices.GoogleLocationService(api);
            var point = GoogleLocationService.GetLatLongFromAddress(replyText);
            string TzIana = GeoTimeZone.TimeZoneLookup.GetTimeZone(point.Latitude, point.Longitude).Result;
            var tzInfo = TimeZoneConverter.TZConvert.GetTimeZoneInfo(TzIana);
            DateTimeOffset dateTimeOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tzInfo);

            string timeRegex = @"\d\d.\d\d";
            Regex timeR = new Regex(timeRegex);
            Match timeMatch = timeR.Match(replyText);
            if (timeMatch.Success)
            {
                string TimeStr = timeMatch.Value;
                var now = DateTime.Now;
                TimeStr = TimeStr.Replace('.', ':');
                var dateTime = DateTime.ParseExact(TimeStr, "H:mm", null, System.Globalization.DateTimeStyles.None);
                if (now > dateTime)
                    dateTime = dateTime.AddDays(1);            
                dateTimeOffset = dateTimeOffset.Add(dateTime - now);
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(dateTimeOffset.ToString(), dateTimeOffset.ToString()), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
