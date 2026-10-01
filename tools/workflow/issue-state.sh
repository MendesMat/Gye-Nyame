#!/usr/bin/env bash
# Estado de uma issue no protocolo das três sessões (docs/workflow/agents.md).
# A label `estado:*` é a fonte da verdade; a coluna do quadro é espelhada no mesmo passo.
#
#   tools/workflow/issue-state.sh status <issue>           mostra estado, bloqueios e PR
#   tools/workflow/issue-state.sh set <issue> <estado>     troca a label e move o cartão
#   tools/workflow/issue-state.sh clear <issue>            devolve ao backlog
#
# Estados: levantamento | pronta-para-execução | em-execução | em-revisão | aprovada
set -euo pipefail

REPO="MendesMat/Gye-Nyame"
PROJECT_OWNER="MendesMat"
PROJECT_NUMBER="1"
STATE_PREFIX="estado:"
DECISION_LABEL="needs-decision"
BACKLOG_COLUMN="Backlog"

column_for_state() {
  case "$1" in
    levantamento)        echo "Levantamento" ;;
    pronta-para-execução) echo "Pronta para execução" ;;
    em-execução)         echo "Em execução" ;;
    em-revisão)          echo "Em revisão" ;;
    aprovada)            echo "Aprovada" ;;
    *) return 1 ;;
  esac
}

fail() { echo "erro: $*" >&2; exit 1; }

require_issue_number() {
  [[ "${1:-}" =~ ^[0-9]+$ ]] || fail "informe o número da issue"
}

current_state() {
  gh issue view "$1" --repo "$REPO" --json labels \
    --jq "[.labels[].name | select(startswith(\"$STATE_PREFIX\")) | ltrimstr(\"$STATE_PREFIX\")] | join(\",\")"
}

has_decision_label() {
  gh issue view "$1" --repo "$REPO" --json labels \
    --jq "[.labels[].name] | index(\"$DECISION_LABEL\") != null"
}

open_blockers() {
  gh api "repos/$REPO/issues/$1/dependencies/blocked_by" \
    --jq '[.[] | select(.state == "open") | "#\(.number)"] | join(" ")'
}

open_pull_request() {
  gh pr list --repo "$REPO" --state open --search "$1 in:body" --json number,body \
    --jq "[.[] | select(.body | test(\"(?i)closes #$1\\\\b\")) | .number] | first // empty"
}

move_card() {
  local issue="$1" column="$2"
  local project_id status_field field_id option_id item_id

  project_id=$(gh project view "$PROJECT_NUMBER" --owner "$PROJECT_OWNER" --format json --jq '.id')
  status_field='.fields[] | select(.name == "Status")'
  field_id=$(gh project field-list "$PROJECT_NUMBER" --owner "$PROJECT_OWNER" --format json \
    --jq "$status_field | .id")
  option_id=$(gh project field-list "$PROJECT_NUMBER" --owner "$PROJECT_OWNER" --format json \
    --jq "$status_field | .options[] | select(.name == \"$column\") | .id")
  item_id=$(gh project item-list "$PROJECT_NUMBER" --owner "$PROJECT_OWNER" --limit 200 --format json \
    --jq ".items[] | select(.content.number == $issue) | .id")

  if [[ -z "$option_id" || -z "$item_id" ]]; then
    echo "aviso: cartão ou coluna '$column' não encontrados; quadro não atualizado" >&2
    return 0
  fi

  gh project item-edit --id "$item_id" --project-id "$project_id" \
    --field-id "$field_id" --single-select-option-id "$option_id" >/dev/null
}

remove_state_labels() {
  local issue="$1" label
  for label in $(gh issue view "$issue" --repo "$REPO" --json labels \
      --jq ".labels[].name | select(startswith(\"$STATE_PREFIX\"))"); do
    gh issue edit "$issue" --repo "$REPO" --remove-label "$label" >/dev/null
  done
}

show_status() {
  local issue="$1" state
  state=$(current_state "$issue")
  echo "issue: #$issue"
  echo "estado: ${state:-backlog}"
  echo "needs-decision: $(has_decision_label "$issue")"
  echo "bloqueadoras abertas: $(open_blockers "$issue")"
  echo "pr aberto: $(open_pull_request "$issue")"
}

set_state() {
  local issue="$1" state="$2" column
  column=$(column_for_state "$state") || fail "estado inválido: $state"
  remove_state_labels "$issue"
  gh issue edit "$issue" --repo "$REPO" --add-label "$STATE_PREFIX$state" >/dev/null
  move_card "$issue" "$column" || echo "aviso: não foi possível mover o cartão" >&2
  echo "#$issue → $state"
}

clear_state() {
  local issue="$1"
  remove_state_labels "$issue"
  move_card "$issue" "$BACKLOG_COLUMN" || echo "aviso: não foi possível mover o cartão" >&2
  echo "#$issue → backlog"
}

command="${1:-}"
case "$command" in
  status) require_issue_number "${2:-}"; show_status "$2" ;;
  set)    require_issue_number "${2:-}"; [[ -n "${3:-}" ]] || fail "informe o estado"; set_state "$2" "$3" ;;
  clear)  require_issue_number "${2:-}"; clear_state "$2" ;;
  *) fail "uso: issue-state.sh status|set|clear <issue> [estado]" ;;
esac
