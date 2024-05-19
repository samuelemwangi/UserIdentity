To generate a private/public key pair, you can use OpenSSL. If you don't have it installed, you can download it from [here](https://www.openssl.org/source/).

1. Open your terminal.

2. Run the following command to generate a private key (Set a good pass phrase!):
```bash
openssl genrsa -aes-256-cbc -passout pass:S3T@G00dPa22Phr@S3 -out private-key.pem 1024
```
 This will generate a 2048 bit RSA private key and save it to a file named private_key.pem.

3. To generate the corresponding public key, run:
 ```
 openssl rsa -pubout -in /private-key.pem -passin pass:S3T@G00dPa22Phr@S3 -outform PEM -out public-key.pem
 ```
 This will read the private key from private_key.pem, generate the corresponding public key, and save it to a file named public_key.pem.
 