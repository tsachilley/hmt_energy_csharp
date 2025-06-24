namespace hmt_energy_csharp;

public static class hmt_energy_csharpDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    public const string UdpNotRegister = "ErrUdp_100001";
    public const string UdpOutofWhitelist = "ErrUdp_100002";
    public const string UdpInsertFailure = "ErrUdp_100003";
    public const string UdpDataInvalid = "ErrUdp_100004";
    public const string UdpProcessError = "ErrUdp_100005";

    public const string TcpNotRegister = "ErrTcp_200001";
    public const string TcpOutofWhitelist = "ErrTcp_200002";
    public const string TcpInsertFailure = "ErrTcp_200003";
    public const string TcpDataInvalid = "ErrTcp_200004";
    public const string TcpProcessError = "ErrTcp_200005";
}