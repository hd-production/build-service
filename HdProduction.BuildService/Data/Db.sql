-- CREATE DATABASE "hd-buildservice";

DROP TABLE IF EXISTS cached_builds;

CREATE TABLE cached_builds (
  build_key VARCHAR(128) PRIMARY KEY,
  date TIMESTAMP NOT NULL,
  self_host_config INT NULL 
);

DROP TABLE IF EXISTS sources_update;

CREATE TABLE sources_update(
  date TIMESTAMP
);
INSERT INTO sources_update VALUES (NOW());
