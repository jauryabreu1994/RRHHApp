using System;

namespace RRHHApp.Controllers
{
    public class Encriptar_DesEncriptar
    {
        public string Encriptar(string _cadenaAencriptar)
        {
            for (int i = 0; i < 2; i++)
            {
                byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
                _cadenaAencriptar = Convert.ToBase64String(encryted);
            }
            return _cadenaAencriptar;
        }

        public string DesEncriptar(string _cadenaAdesencriptar)
        {
            for (int i = 0; i < 2; i++)
            {
                byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
                _cadenaAdesencriptar = System.Text.Encoding.Unicode.GetString(decryted);
            }
            return _cadenaAdesencriptar;
        }
    }
}