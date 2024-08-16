using GrpcService.Notify;
using GrpcService.Model;

namespace GrpcService.IGrpc;

public interface IEmailGrpc
{
    Task<bool> SendSimpleMail(EmailRequestSimpleModel request);
}