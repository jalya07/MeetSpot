#!/usr/bin/env bash
set -euo pipefail

# Back up the MeetSpot SQL Server database running in Docker.
# Usage: SQL_SA_PASSWORD=<pass> ./scripts/backup-sql.sh [backup_dir]

BACKUP_DIR="${1:-./backups}"
CONTAINER="${CONTAINER:-meetspot-sql}"
DATABASE="${DATABASE:-MeetSpotDb}"
SQL_SA_PASSWORD="${SQL_SA_PASSWORD:-}"

if [[ -z "$SQL_SA_PASSWORD" ]]; then
    echo "Set SQL_SA_PASSWORD environment variable" >&2
    exit 1
fi

mkdir -p "$BACKUP_DIR"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
FILE="MeetSpotDb_${TIMESTAMP}.bak"
LOCAL_PATH="$BACKUP_DIR/$FILE"

docker exec "$CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "$SQL_SA_PASSWORD" -No -Q \
    "BACKUP DATABASE [$DATABASE] TO DISK = N'/tmp/$FILE' WITH FORMAT, COMPRESSION"

docker cp "$CONTAINER:/tmp/$FILE" "$LOCAL_PATH"
docker exec "$CONTAINER" rm "/tmp/$FILE"

echo "Backup saved to $LOCAL_PATH"
