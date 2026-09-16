# Entorno de Infraestructura Local

Este directorio contiene la configuración de **Docker Compose** para levantar la infraestructura base necesaria para el desarrollo local del proyecto SIA.

## Requisitos Previos

Antes de iniciar el entorno local, es necesario contar con:

* [Docker](https://docs.docker.com/get-docker/) instalado.
* Docker Compose.

En versiones actuales de Docker Desktop, Docker Compose está disponible mediante el comando `docker compose`.

## Componentes y Puertos

| Componente         | Contenedor            | Puerto(s) local | Usuario | Contraseña         |
| :------------------| :-------------------- | :-------------- | :------ | :----------------- |
| SQL Server 2022    | `sia_sqlserver_local` | `1433`          | `sa`    | `Sia_Dev_P@ssw0rd` |
| RabbitMQ Broker    | `sia_rabbitmq_local`  | `5672`          | `guest` | `guest`            |
| RabbitMQ Management| `sia_rabbitmq_local`  | `15672`         | `guest` | `guest`            |

### RabbitMQ Management

La interfaz de administración de RabbitMQ está disponible desde el navegador en: http://localhost:15672


## Volúmenes Persistentes

El archivo `docker-compose.yml` configura los siguientes volúmenes nombrados:

| Volumen              | Servicio   |
| :------------------- | :--------- |
| `sia_sqlserver_data` | SQL Server |
| `sia_rabbitmq_data`  | RabbitMQ   |

Estos volúmenes permiten conservar los datos de SQL Server y RabbitMQ aunque los contenedores sean detenidos o eliminados.

> **Nota:** los datos almacenados en los volúmenes se eliminarán si estos son eliminados explícitamente mediante Docker.

## Comandos de Uso

### Levantar la infraestructura

Abre una terminal en este directorio (`deploy/compose/`) y ejecuta: `docker compose up -d`

El parámetro `-d` ejecuta los contenedores en segundo plano.

Verificar el estado de los contenedores : `docker compose ps`

### Ver los logs

Para consultar los logs de todos los servicios: `docker compose logs`


Para consultar los logs de un servicio específico:

`docker compose logs sia-sqlserver`
`docker compose logs sia-rabbitmq`

### Detener la infraestructura

Para detener los servicios: `docker compose stop`

Los contenedores y los volúmenes se conservan.

Detener y eliminar los contenedores: `docker compose down`

Esto elimina los contenedores, pero mantiene los volúmenes persistentes.

### Eliminar contenedores y volúmenes

Si se requiere eliminar completamente el entorno y todos los datos persistidos: `docker compose down -v`

> **Advertencia:** este comando elimina los volúmenes `sia_sqlserver_data` y `sia_rabbitmq_data`, por lo que se perderán los datos almacenados en SQL Server y RabbitMQ.
