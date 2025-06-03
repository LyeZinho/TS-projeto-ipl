using System;
using System.Text;
using System.Security.Cryptography;

public class RsaChatCrypto
{
    // Gera um novo par de chaves RSA (2048 bits)
    public static (byte[] publicKey, byte[] privateKey) GerarParDeChaves()
    {
        // Cria um novo par de chaves RSA
        using RSA rsa = RSA.Create(2048);
        // Exporta a chave pública no formato SubjectPublicKeyInfo
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
        byte[] privateKey = rsa.ExportPkcs8PrivateKey();
        return (publicKey, privateKey);
    }

    // Encripta uma mensagem usando a chave pública (formato SubjectPublicKeyInfo)
    public static byte[] Encriptar(string mensagem, byte[] chavePublica)
    {
        // Cria um novo objeto RSA e importa a chave pública
        using RSA rsa = RSA.Create();
        // Importa a chave pública no formato SubjectPublicKeyInfo
        rsa.ImportSubjectPublicKeyInfo(chavePublica, out _);
        // Transforma a mensagem em bytes e a encripta
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

Funciona com encriptação RSA.

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
