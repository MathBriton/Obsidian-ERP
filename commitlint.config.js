/**
 * Padrão de commits do Obsidian ERP — Conventional Commits.
 * Tipo em inglês (feat, fix, chore, ...), descrição em pt-BR.
 * Ex.: "feat(orders): adiciona cancelamento de pedido"
 */
export default {
  extends: ["@commitlint/config-conventional"],
  rules: {
    "type-enum": [
      2,
      "always",
      ["feat", "fix", "docs", "style", "refactor", "perf", "test", "build", "ci", "chore", "revert"],
    ],
    "subject-case": [0],
    "header-max-length": [2, "always", 100],
  },
}
