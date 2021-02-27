# Web Services

> A web service is a software system designed to support interoperable machine to machine interaction over a network (W3C)... A web service must have a described interface and other systems interact with this web service using Simple Object Access Protocol (SOAP) tipically over HTTP using XML serialization.

# E01-WebServiceTask.dtsx

Contiene un ejemplo de las tareas *Web Service Task*, *XML Task*, *XML Source*.

## Web Service Task

Esta tarea es útil cuando el archivo que contiene la descripción del servicio web (extensión *wsdl*) se puede obtener a través de una conexión HTTP. Para obtenerlo mediante conexiones seguras como HTTPS se requiere un certificado, y credenciales apropiados.

En la primer pantalla de configuración, se establece la conexión al servicio creando un *connection manager* e introduciendo la dirección HTTP (u otras conexiones seguras siempre y cuando existan los credenciales apropiados). Luego, se debe escoger la conexión al archivo *wsdl* **que previamente debe ser creado** (puede ser usando notepad, haciendo un documento nuevo en blanco y cambiando la extensión del archivo). Configurar la opción *OverwriteWSDLFile* a `TRUE`. Posteriormente, hacer clic en `Download WSDL`.

Como resultado de esa operación el archivo *wsdl* previamente vacío, ahora debe tener la descripción del servicio usando serialización XML.

En la pestaña *Input* (segunda pantalla de configuración), se escoge el servicio deseado. Luego, se escoge el método y los parámetros del servicio. Para saber qué servicio, método y parámetros son necesarios, hay que leer la documentación del proveedor.

En la tercer pantalla *Output*, se escoge el archivo de destino que contendrá los datos. El archivo usualmente es un XML totalmente en blanco, que fue **creado previamente durante el diseño del paquete**. Existe la opción de que la conexión sea por medio de una variable. El archivo se sobreescribe por defecto cada vez que se ejecuta el paquete.

## XML Task

Completado el paso anterior, el resultado debe ser que el XML en blanco, ahora contiene los datos requeridos mediante el servicio. Usualmente, esos datos están acompañados de elementos que contienen *namespaces*. Esto puede bloquear el componente *XML Source* dentro del *Data Flow Task*.

La razón de que esto pueda ser un problema se da, porque el archivo XML debe estar acompañado de un esquema (XSD). Si el proveedor brinda el esquema (XSD) es posible que el XML descargado por el servicio pueda ser usado directamente. De lo contrario, SSIS debe generar un esquema a partir del XML.

Si SSIS trata de generar un esquema a partir del XML, pero este contiene *namespaces* lanzará un error y la ejecución se detiene. Por tanto, el XML descargado usando el servicio WEB debe limpiarse.

Para ello, se utiliza el componente *XML Task*. Este componente permite realizar varias operaciones a un XML, como: differenciar, Validar, XPath, o XSLT. La última es una transformación del archivo XML usando instrucciones contenidas en un archivo con extensión XSLT. Por tanto, para ello se deben tener los siguientes archivos:

- *XML Original*: El archivo descargado usando el servicio web. Este contiene los *namespaces* problemáticos.
- *XML en blanco*: El archivo que recibirá el resultado de la transformación. Contiene los mismos datos que el XML original pero **sin** los *namespaces*.
- *XSLT*: El archivo con las instrucciones de la transformación. Es un archivo con serialización XML, pero contiene instrucciones. Para eliminar los namespaces, el archivo usado fue obtenido de la internet.

Para configurar el *XML Task*, ir a la pestaña *General*:

1. En *Input*, en la opción *OperationType*, escoger: `XSLT`.
1. En la opción *SourceType*: usar una conexión de archivo que apunte al *XML Original* (opción *Source*).
1. En *Output*, 
   - En la opción *SaveOperationResult* usar `True`.
   - En *DestinationType* usar un `FileConnection`.
   - En *Destination* apuntar al *XML en blanco*.
   - en *OverwriteDestination* usar `True`
1. En *Second Operand*:
   - En *SecondOperandType* usar `File Connection`.
   - En *SecondOperand* apuntar al archivo *XSLT*.

## Data Flow

### XML Source

Ahora que el archivo XML tiene un formato utilizable, se puede empezar con el *Data Flow*. Luego, agregar un *XML Source*.

Este componente se utiliza para escoger el archivo XML con los datos que se desean importar. Pero también tiene la opción para generar un esquema XML (XSD) dando click al botón `Generate XSD`. Este generará un archivo XSD que permite interpretar el archivo XML con los datos.

Después de este paso, se mapean las columnas, se transforman los datos si es necesario, y se envían al resto del proceso de ETL.