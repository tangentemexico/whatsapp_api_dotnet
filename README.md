# whatsapp_api_dotnet
<h1>Servidor de mensajeria por Whatsapp</h1><br/>
Api para mensajeria de whatsapp <br/>
Requiere otro proyecto: 
https://github.com/tangentemexico/whatsapp_api_node<br/>
<br/>
Pasos:<br/>
1. Levantar el proyecto node <br/>
2. Montar tu base de datos <br/>
3. Correr el proyecto .net<br/>
4. Revisa la carpeta Postman para ver las opciones de petición.<br/>
<br/>
<br/>
FLUJO INTERNO.<br/>
1. Recibe la petición<br/>
2. Valida usuario y contraseña<br/>
3. Guarda el mensaje en tablas <br/>
4. En un hilo independiente envia los mensajes guardados <br/>
5. En caso que haya varios mensajes, espera 3 segundos entre cada mensaje.<br/>
6. En caso que no se pudo enviar, la tabla guarda el numero de intentos realizados, después de 10 intentos ya no lo envia.<br/>
<br/>
<br/>
<b>NOTAS</b><br/>
Puedes configurar varios némeros de whatsapp usando diferentes puertos<br/>
en la tabla usuario_servicio podras configurar los puertos para cada usuario<br/>
<br/>
FINALMENTE<br/>
Este código no es para que lo uses en productivo (aunque si funciona) <br/>
La intención es que veas como implementar tu propio servidor de mensajeria por whatsapp.<br/>
RECUERDA <br/>
El tiempo de 3 segundos es para que whatsapp no vaya bloquear tu número <br/><br/>
Descargo de responsabilidad<br/>
Este proyecto proporciona ejemplos de código no oficiales para interactuar con la API de WhatsApp. No está afiliado, aprobado ni respaldado por Meta Platforms, Inc.<br/>
<br/>
El uso de este software es bajo tu propia responsabilidad. Ni el autor ni los contribuyentes se hacen responsables de ningún daño, pérdida de datos, interrupción de servicio o reclamación legal que pueda derivarse de su uso, incluyendo pero no limitado al uso de canales o métodos no oficiales para el envío de mensajes a través de WhatsApp.<br/>
<br/>
Se recomienda enfáticamente revisar los términos de servicio y políticas de Meta/WhatsApp antes de desplegar este código en entornos de producción.<br/>
