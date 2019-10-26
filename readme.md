#  Logging Sample  
Projeto para testar e comparar a implementação de soluções de log do .Net
- Utilizado .Net Core 2.2 e Full Framework 4.6.1
- VsualStudio Profiler
- SeriLog
- NLog
- MongoDB 
- Elaisticsearch
- Docker
- Flurl
- Nest
- Polly
 
###  Objetivo  
Verificar a facilidade e impacto das implementações de log utilizando bibliotecas e fazendo manualmente


###  Conclusão  
A implementação do Serilog é mais simples e eficiente que a do NLog pois pode ser feita diretamente no código sem a necessidade de um XML como o NLog.
A implementação feita manualmente com http para o meu caso se mostrou mais eficiente no consumo de recursos e na customização do objeto de log e com maior controle sobre o nome da collection ou indice que vai ser utilizado para armazenar.
  


###  Referências  
[Busitec blog](https://blog.busitec.de/fachliches-und-technisches-logging-mit-serilog/)
[NLog](https://nlog-project.org/)
[SeriLog](https://serilog.net/)
[Sample CSV DATA](https://support.spatialkey.com/spatialkey-sample-csv-data/)
[Serilog](https://serilog.net/)
[Nest](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/introduction.html)
[Mongo Driver](https://docs.mongodb.com/ecosystem/drivers/csharp/)
[Flurl](https://flurl.dev/docs/fluent-url/)
[Polly](https://github.com/App-vNext/Polly)
