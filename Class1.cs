using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    private async Task<int> HandleUserInAppAbuseReportEventModel<T>(string messageBody)
    {
        try
        {
            
            var theEvent = DeserializeMessage<T>(messageBody);
            if (theEvent == null)
            {
                return NonEffectNum;
            }
            
            var exists = await _AiModerationRepository.GetByEventId<T>(theEvent.EventId).ConfigureAwait(false);
            if (exists != null)
            {
                return NonEffectNum;
            }
            
            var res = SaveUserInAppAbuseReportEventData(userInAppAbuseReportEvent);
            
            await SendEventDataToTickerMaker(userInAppAbuseReportEvent).ConfigureAwait(false);
            return res;
        }
        catch (Exception ex)
        {
            _Logger.LogError("Event Processor Job received exception inside HandleUserUpdatedEventModel.");
            throw ex;
        }
    }
}
