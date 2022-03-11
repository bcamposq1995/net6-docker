# POSTGRES
```bash
docker run --rm --name postgres-dev -e POSTGRES_PASSWORD=123456789 -e POSTGRES_USER=person -e POSTGRES_DB=person -p 5432:5432 -d postgres
```

# REDIS
```bash
docker run --rm --name redis-dev -p 6379:6379 -d redis
```

# PROMETHEUS
```bash
docker run --rm --name prometheus-dev --add-host host.docker.internal:host-gateway -p 9090:9090 -v $(pwd)/prometheus/config/prometheus-dev.yml:/etc/prometheus/prometheus.yml -d prom/prometheus
```

# RABBIT
```bash
docker run --rm --name redis-dev -p 6379:6379 -d redis
```