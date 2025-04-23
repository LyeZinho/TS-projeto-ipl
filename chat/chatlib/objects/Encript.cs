using System;
using System.Text;
using System.Security.Cryptography;

public class RsaChatCrypto
{
    // Gera um novo par de chaves RSA (2048 bits)
    public static (byte[] publicKey, byte[] privateKey) GerarParDeChaves()
    {
        using RSA rsa = RSA.Create(2048);
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
        byte[] privateKey = rsa.ExportPkcs8PrivateKey();
        return (publicKey, privateKey);
    }

    // Encripta uma mensagem usando a chave pública (formato SubjectPublicKeyInfo)
    public static byte[] Encriptar(string mensagem, byte[] chavePublica)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(chavePublica, out _);
        byte[] dados = Encoding.UTF8.GetBytes(mensagem);
        return rsa.Encrypt(dados, RSAEncryptionPadding.Pkcs1);
    }

    // Desencripta dados usando a chave privada (formato PKCS8)
    public static string Desencriptar(byte[] dadosCriptografados, byte[] chavePrivada)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(chavePrivada, out _);
        byte[] dados = rsa.Decrypt(dadosCriptografados, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(dados);
    }
}

/*
 Guia

        // Usuário B gera suas chaves
        var (chavePublicaB, chavePrivadaB) = RsaChatCrypto.GerarParDeChaves();

        // Usuário A escreve uma mensagem e encripta com a chave pública de B
        string mensagemDeA = "Olá, esta é uma mensagem secreta!";
        byte[] mensagemCriptografada = RsaChatCrypto.Encriptar(mensagemDeA, chavePublicaB);

        Console.WriteLine("Mensagem Criptografada (Base64):");
        Console.WriteLine(Convert.ToBase64String(mensagemCriptografada));

        // Usuário B desencripta com sua chave privada
        string mensagemDescriptografada = RsaChatCrypto.Desencriptar(mensagemCriptografada, chavePrivadaB);

        Console.WriteLine("\nMensagem Descriptografada:");
        Console.WriteLine(mensagemDescriptografada);

 */
