using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace IdentityServer.Custom
{
    public class KeyManagementService(string jwksPath)
    {
        public JsonWebKeySet LoadOrCreateKeys()
        {
            if (File.Exists(jwksPath))
            {
                var json = File.ReadAllText(jwksPath);
                return JsonSerializer.Deserialize<JsonWebKeySet>(json);
            }
            else
            {
                var keys = GenerateKeys();
                File.WriteAllText(jwksPath, JsonSerializer.Serialize(keys));
                return keys;
            }
        }

        private JsonWebKeySet GenerateKeys()
        {
            var jwks = new JsonWebKeySet();

            var rsaKey = new RsaSecurityKey(RSA.Create(2048))
            {
                KeyId = "sig-rs-0",
            };
            var rsaJwk = JsonWebKeyConverter.ConvertFromSecurityKey(rsaKey);
            rsaJwk.Use = "sig";
            jwks.Keys.Add(rsaJwk);

            var rsaEncKey = new RsaSecurityKey(RSA.Create(2048))
            {
                KeyId = "enc-rs-0"
            };
            var rsaEncJwk = JsonWebKeyConverter.ConvertFromSecurityKey(rsaEncKey);
            rsaEncJwk.Use = "enc";
            jwks.Keys.Add(rsaEncJwk);

            var ecKeyP256 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                KeyId = "sig-ec2-0"
            };
            var ecJwkP256 = JsonWebKeyConverter.ConvertFromSecurityKey(ecKeyP256);
            ecJwkP256.Use = "sig";
            jwks.Keys.Add(ecJwkP256);

            var ecEncKeyP256 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                KeyId = "enc-ec2-0"
            };
            var ecEncJwkP256 = JsonWebKeyConverter.ConvertFromSecurityKey(ecEncKeyP256);
            ecEncJwkP256.Use = "enc";
            jwks.Keys.Add(ecEncJwkP256);

            var ecKeyP384 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP384))
            {
                KeyId = "sig-ec3-0"
            };
            var ecJwkP384 = JsonWebKeyConverter.ConvertFromSecurityKey(ecKeyP384);
            ecJwkP384.Use = "sig";
            jwks.Keys.Add(ecJwkP384);

            var ecEncKeyP384 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP384))
            {
                KeyId = "enc-ec3-0"
            };
            var ecEncJwkP384 = JsonWebKeyConverter.ConvertFromSecurityKey(ecEncKeyP384);
            ecEncJwkP384.Use = "enc";
            jwks.Keys.Add(ecEncJwkP384);

            var ecKeyP521 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP521))
            {
                KeyId = "sig-ec5-0"
            };
            var ecJwkP521 = JsonWebKeyConverter.ConvertFromSecurityKey(ecKeyP521);
            ecJwkP521.Use = "sig";
            jwks.Keys.Add(ecJwkP521);

            var ecEncKeyP521 = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP521))
            {
                KeyId = "enc-ec5-0"
            };
            var ecEncJwkP521 = JsonWebKeyConverter.ConvertFromSecurityKey(ecEncKeyP521);
            ecEncJwkP521.Use = "enc";
            jwks.Keys.Add(ecEncJwkP521);

            return jwks;
        }
    }
}