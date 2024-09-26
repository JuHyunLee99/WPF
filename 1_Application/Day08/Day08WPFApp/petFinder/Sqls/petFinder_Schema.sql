truncate animalinfo;CREATE TABLE animalinfo (
  Idx int NOT NULL AUTO_INCREMENT,
  Sj varchar(300) NOT NULL,
  WritngDe date DEFAULT NULL,
  Cn varchar(300) DEFAULT NULL,
  Ty3Date varchar(45) DEFAULT NULL,
  Ty3Place varchar(300) DEFAULT NULL,
  Ty3Kind varchar(45) DEFAULT NULL,
  Ty3Sex varchar(45) DEFAULT NULL,
  Ty3Process varchar(45) DEFAULT NULL,
  Ty3Ingye varchar(100) DEFAULT NULL,
  Ty3Insu varchar(100) DEFAULT NULL,
  Ty3Picture varchar(300) DEFAULT NULL,
  PRIMARY KEY (Idx)
)
