VsCSharpWinForm_sample2

It is a Visual Studio C Sharp Windows Form sample code version 2, which demonstrate the following common codes.

DbHelper
ExcelHelper
FileHelper
GeneralT
MailHelper
Singleton
TIniFile
TLog
TTcpClientSocket
TTcpServerSocket
TTcpSocket
Login Process

Data transfer process in TTcpClientSocket, TTcpServerSocket and TTcpSocket:
Sender:
Convert data in byte array no matter which data is, such as text, file, or heartbeat.
Encrypt data as an encrypted data in byte array.
Socket packs the encrypted data into a data package.
Socket sends the data package in byte array to another socket.

Data (e.g. text, file or heartbeat)  +---------------+ Data in byte array  +------------+ Encrypted Data  +---------------+ Data Package  +-----------------+
-----------------------------------> | Serialization | ------------------> | Encryption | --------------> | Sender Socket | ------------> | Receiver Socket |
                                     +---------------+                     +------------+                 +---------------+               +-----------------+
Receiver:
Socket receives the data package in byte array to another socket.
Socket unpacks the data package to the data in byte array.
Decrypt data as a decrypted data in byte array.
Convert data in byte array into its original form, such as text, file, heartbeat.

+---------------+ Data Package  +-----------------+ Encrypted Data  +------------+ Decrypted Data  +-----------------+ Data (e.g. text, file or heartbeat)
| Sender Socket | ------------> | Receiver Socket | --------------> | Decryption | --------------> | Deserialization | ----------------------------------->
+---------------+               +-----------------+                 +------------+                 +-----------------+

TTcpSocket is a combination of TTcpClientSocket and TTcpServerSocket.
2021-04-15 Can specify the ExternalActToHandleIncomingData parameter for external function to handle the incoming data.

Text Serialization
[1][Content]

File Serialization
[2][Length of filename (4 bytes)][Filename in byte array][Last index of pieces (4 bytes)][Index of pieces starting from 0 (4 bytes)][File content]

[Done] * Merge the socket server and client into one file.
[Done] * Convert file to byte array, including filename, and reverse.
[Done in file, no need in text] * Split a longer byte array into several byte arrays, and merge them into a single byte array.
* OAuth

