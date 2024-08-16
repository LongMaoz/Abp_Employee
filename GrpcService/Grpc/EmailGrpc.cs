using AutoMapper;
using Grpc.Net.Client;
using GrpcService.Notify;
using Config;
using GrpcService.IGrpc;
using GrpcService.Model;
using Microsoft.Extensions.Options;

namespace GrpcService.Grpc;

public class EmailGrpc:IEmailGrpc
{
    GrpcErpApi _grpcErpApi;
    GrpcHelper _grpcHelper;
    GrpcChannel Channel { get; set; }
    IMapper _mapper;
    const string _cacheKey = "EmialGrpc";
    public EmailGrpc(IOptionsSnapshot<GrpcErpApi> grpcErpApi, GrpcHelper grpcHelper, IMapper mapper)
    {
        _grpcErpApi = grpcErpApi.Value;
        _grpcHelper = grpcHelper;
        _mapper = mapper;
    }
    protected async Task<GrpcChannel> GetChannel()
    {
        Channel ??= await _grpcHelper.GetGrpcChannel(typeof(EmailGrpc), _grpcErpApi.NotifyKey);
        return this.Channel;
    }


    public async Task<bool> SendSimpleMail(EmailRequestSimpleModel request)
    {
        EmailSender.EmailSenderClient client = new EmailSender.EmailSenderClient(await GetChannel());
        var result = await client.SendEmailAsync(_mapper.Map<EmailSimpleRequest>(request));
        return result.Result;
    }

}