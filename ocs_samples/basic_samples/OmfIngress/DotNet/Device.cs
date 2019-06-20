﻿using OSIsoft.Data.Http;
using OSIsoft.Identity;
using OSIsoft.Omf;
using OSIsoft.OmfIngress;
using System;
using System.Threading.Tasks;

namespace OmfIngressClientLibraries
{
    internal class Device
    {
        private readonly IOmfIngressService _deviceOmfIngressService;

        public Device(string address, string tenantId, string namespaceId, string clientId, string clientSecret)
        {
            // Create the AuthenticationHandler and IngressSerice to use to send data
            AuthenticationHandler deviceAuthenticationHandler = new AuthenticationHandler(new Uri(address), clientId, clientSecret);

            OmfIngressService deviceBaseOmfIngressService = new OmfIngressService(new Uri(address), null, HttpCompressionMethod.None, deviceAuthenticationHandler);
            _deviceOmfIngressService = deviceBaseOmfIngressService.GetOmfIngressService(tenantId, namespaceId);
        }

        public async Task CreateDataPointTypeAsync()
        {
            // Create a DataPointType
            Console.WriteLine($"Creating Type with Id {typeof(DataPointType).Name}");
            Console.WriteLine();

            OmfTypeMessage typeMessage = OmfMessageCreator.CreateTypeMessage(typeof(DataPointType));
            SerializedOmfMessage serializedTypeMessage = OmfMessageSerializer.Serialize(typeMessage);
            await _deviceOmfIngressService.SendOMFMessageAsync(serializedTypeMessage);
        }

        public async Task CreateStreamAsync(string streamId)
        {
            // Create container
            Console.WriteLine($"Creating Container with Id {streamId}");
            Console.WriteLine();

            OmfContainerMessage containerMessage = OmfMessageCreator.CreateContainerMessage(streamId, typeof(DataPointType));
            SerializedOmfMessage serializedContainerMessage = OmfMessageSerializer.Serialize(containerMessage);
            await _deviceOmfIngressService.SendOMFMessageAsync(serializedContainerMessage);
        }

        public async Task SendValueAsync(string streamId, DataPointType value)
        {
            // Send DataPointType values
            OmfDataMessage dataMessage = OmfMessageCreator.CreateDataMessage(streamId, value);
            SerializedOmfMessage serializedDataMessage = OmfMessageSerializer.Serialize(dataMessage);

            await _deviceOmfIngressService.SendOMFMessageAsync(serializedDataMessage);
            Console.WriteLine($"Sent data point: Time: {value.Timestamp}, Value: {value.Value}");
        }

        public async Task DeleteDataPointTypeAsync()
        {
            // Delete type
            Console.WriteLine($"Deleting Type with Id {typeof(DataPointType).Name}");
            Console.WriteLine();
            OmfTypeMessage typeMessage = OmfMessageCreator.CreateTypeMessage(typeof(DataPointType));
            typeMessage.ActionType = ActionType.Delete;
            SerializedOmfMessage serializedTypeMessage = OmfMessageSerializer.Serialize(typeMessage);
            await _deviceOmfIngressService.SendOMFMessageAsync(serializedTypeMessage);
        }

        public async Task DeleteStreamAsync(string streamId)
        {
            // Delete container
            Console.WriteLine($"Deleting Container with Id {streamId}");
            Console.WriteLine();
            OmfContainerMessage containerMessage = OmfMessageCreator.CreateContainerMessage(streamId, typeof(DataPointType));
            containerMessage.ActionType = ActionType.Delete;
            SerializedOmfMessage serializedContainerMessage = OmfMessageSerializer.Serialize(containerMessage);           
            await _deviceOmfIngressService.SendOMFMessageAsync(serializedContainerMessage);  
        }        
    }
}
