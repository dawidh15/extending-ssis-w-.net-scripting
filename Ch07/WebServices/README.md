# Web Services

> A web service is a software system designed to support interoperable machine to machine interaction over a network (W3C)... A web service must have a described interface and other systems interact with this web service using Simple Object Access Protocol (SOAP) tipically over HTTP using XML serialization.

# E01-WebServiceTask.dtsx

Contiene un ejemplo de las tareas *Web Service Task*, *XML Task*, *XML Source*.

## Web Service Task

Esta tarea es �til cuando el archivo que contiene la descripci�n del servicio web (extensi�n *wsdl*) se puede obtener a trav�s de una conexi�n HTTP. Para obtenerlo mediante conexiones seguras como HTTPS se requiere un certificado, y credenciales apropiados.

En la primer pantalla de configuraci�n, se establece la conexi�n al servicio creando un *connection manager* e introduciendo la direcci�n HTTP (u otras conexiones seguras siempre y cuando existan los credenciales apropiados). Luego, se debe escoger la conexi�n al archivo *wsdl* **que previamente debe ser creado** (puede ser usando notepad, haciendo un documento nuevo en blanco y cambiando la extensi�n del archivo). Configurar la opci�n *OverwriteWSDLFile* a `TRUE`. Posteriormente, hacer clic en `Download WSDL`.

Como resultado de esa operaci�n el archivo *wsdl* previamente vac�o, ahora debe tener la descripci�n del servicio usando serializaci�n XML.

En la pesta�a *Input* (segunda pantalla de configuraci�n), se escoge el servicio deseado. Luego, se escoge el m�todo y los par�metros del servicio. Para saber qu� servicio, m�todo y par�metros son necesarios, hay que leer la documentaci�n del proveedor.

En la tercer pantalla *Output*, se escoge el archivo de destino que contendr� los datos. El archivo usualmente es un XML totalmente en blanco, que fue **creado previamente durante el dise�o del paquete**. Existe la opci�n de que la conexi�n sea por medio de una variable. El archivo se sobreescribe por defecto cada vez que se ejecuta el paquete.

## XML Task

Completado el paso anterior, el resultado debe ser que el XML en blanco, ahora contiene los datos requeridos mediante el servicio. Usualmente, esos datos est�n acompa�ados de elementos que contienen *namespaces*. Esto puede bloquear el componente *XML Source* dentro del *Data Flow Task*.

La raz�n de que esto pueda ser un problema se da, porque el archivo XML debe estar acompa�ado de un esquema (XSD). Si el proveedor brinda el esquema (XSD) es posible que el XML descargado por el servicio pueda ser usado directamente. De lo contrario, SSIS debe generar un esquema a partir del XML.

Si SSIS trata de generar un esquema a partir del XML, pero este contiene *namespaces* lanzar� un error y la ejecuci�n se detiene. Por tanto, el XML descargado usando el servicio WEB debe limpiarse.

Para ello, se utiliza el componente *XML Task*. Este componente permite realizar varias operaciones a un XML, como: differenciar, Validar, XPath, o XSLT. La �ltima es una transformaci�n del archivo XML usando instrucciones contenidas en un archivo con extensi�n XSLT. Por tanto, para ello se deben tener los siguientes archivos:

- *XML Original*: El archivo descargado usando el servicio web. Este contiene los *namespaces* problem�ticos.
- *XML en blanco*: El archivo que recibir� el resultado de la transformaci�n. Contiene los mismos datos que el XML original pero **sin** los *namespaces*.
- *XSLT*: El archivo con las instrucciones de la transformaci�n. Es un archivo con serializaci�n XML, pero contiene instrucciones. Para eliminar los namespaces, el archivo usado fue obtenido de la internet.

Para configurar el *XML Task*, ir a la pesta�a *General*:

1. En *Input*, en la opci�n *OperationType*, escoger: `XSLT`.
1. En la opci�n *SourceType*: usar una conexi�n de archivo que apunte al *XML Original* (opci�n *Source*).
1. En *Output*, 
   - En la opci�n *SaveOperationResult* usar `True`.
   - En *DestinationType* usar un `FileConnection`.
   - En *Destination* apuntar al *XML en blanco*.
   - en *OverwriteDestination* usar `True`
1. En *Second Operand*:
   - En *SecondOperandType* usar `File Connection`.
   - En *SecondOperand* apuntar al archivo *XSLT*.

## Data Flow

### XML Source

Ahora que el archivo XML tiene un formato utilizable, se puede empezar con el *Data Flow*. Luego, agregar un *XML Source*.

Este componente se utiliza para escoger el archivo XML con los datos que se desean importar. Pero tambi�n tiene la opci�n para generar un esquema XML (XSD) dando click al bot�n `Generate XSD`. Este generar� un archivo XSD que permite interpretar el archivo XML con los datos.

Despu�s de este paso, se mapean las columnas, se transforman los datos si es necesario, y se env�an al resto del proceso de ETL.