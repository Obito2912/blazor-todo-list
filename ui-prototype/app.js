const initialTasks = [
  { id: 1, title: "Review project proposal", description: "Confirm the final scope and UI requirements with the team.", dueDate: "2026-07-18", completed: false },
  { id: 2, title: "Prepare dashboard components", description: "Define the task row, filters, form, and reusable empty state.", dueDate: "2026-07-19", completed: false },
  { id: 3, title: "Share design options", description: "Collect votes and comments for the five visual directions.", dueDate: "2026-07-17", completed: true },
  { id: 4, title: "Test the mobile layout", description: "Check navigation and task controls at phone and tablet widths.", dueDate: "2026-07-20", completed: false },
  { id: 5, title: "Document component behavior", description: "Record parameters, events, validation, and UI states.", dueDate: "2026-07-21", completed: false }
];

let tasks = structuredClone(initialTasks);
let activeFilter = "all";
let taskToDelete = null;

const elements = {
  taskList: document.querySelector("#taskList"), loading: document.querySelector("#loadingState"), empty: document.querySelector("#emptyState"),
  search: document.querySelector("#searchInput"), form: document.querySelector("#taskForm"), dialog: document.querySelector("#taskDialog"),
  deleteDialog: document.querySelector("#deleteDialog"), title: document.querySelector("#taskTitle"), description: document.querySelector("#taskDescription"),
  dueDate: document.querySelector("#taskDueDate"), taskId: document.querySelector("#taskId"), dialogTitle: document.querySelector("#dialogTitle"),
  dialogEyebrow: document.querySelector("#dialogEyebrow"), titleError: document.querySelector("#titleError"), dateError: document.querySelector("#dateError"),
  sidebar: document.querySelector("#sidebar"), overlay: document.querySelector("#mobileOverlay"), menu: document.querySelector("#menuButton"), toast: document.querySelector("#toast")
};

const escapeHtml = value => value.replace(/[&<>'"]/g, char => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "'": "&#39;", '"': "&quot;" })[char]);
const formatDate = date => new Intl.DateTimeFormat("en", { month: "short", day: "numeric", year: "numeric" }).format(new Date(`${date}T12:00:00`));

function visibleTasks() {
  const query = elements.search.value.trim().toLowerCase();
  return tasks.filter(task => {
    const matchesFilter = activeFilter === "all" || (activeFilter === "completed" ? task.completed : !task.completed);
    const matchesQuery = !query || `${task.title} ${task.description}`.toLowerCase().includes(query);
    return matchesFilter && matchesQuery;
  });
}

function renderTasks() {
  const visible = visibleTasks();
  elements.taskList.innerHTML = visible.map(task => `
    <article class="task-row ${task.completed ? "completed" : ""}" data-id="${task.id}">
      <button class="complete-button" type="button" data-action="toggle" aria-label="${task.completed ? "Mark pending" : "Mark completed"}">${task.completed ? "✓" : ""}</button>
      <div class="task-copy"><h3>${escapeHtml(task.title)}</h3><p>${escapeHtml(task.description || "No description")}</p></div>
      <div class="task-meta"><span aria-hidden="true">▣</span><time datetime="${task.dueDate}">${formatDate(task.dueDate)}</time></div>
      <div class="task-actions">
        <button class="icon-button" type="button" data-action="edit" aria-label="Edit ${escapeHtml(task.title)}">✎</button>
        <button class="icon-button delete-action" type="button" data-action="delete" aria-label="Delete ${escapeHtml(task.title)}">♲</button>
      </div>
    </article>`).join("");
  elements.empty.hidden = visible.length > 0;
  document.querySelector("#resultSummary").textContent = `${visible.length} ${visible.length === 1 ? "task" : "tasks"} shown`;
  document.querySelector("#allCount").textContent = tasks.length;
  document.querySelector("#pendingCount").textContent = tasks.filter(task => !task.completed).length;
  document.querySelector("#completedCount").textContent = tasks.filter(task => task.completed).length;
}

function openTaskDialog(task = null) {
  elements.form.reset(); elements.titleError.textContent = ""; elements.dateError.textContent = "";
  elements.taskId.value = task?.id ?? ""; elements.title.value = task?.title ?? ""; elements.description.value = task?.description ?? "";
  elements.dueDate.value = task?.dueDate ?? new Date(Date.now() + 86400000).toISOString().slice(0, 10);
  elements.dialogTitle.textContent = task ? "Edit Task" : "Add Task"; elements.dialogEyebrow.textContent = task ? "Update quest" : "New quest";
  elements.dialog.showModal(); setTimeout(() => elements.title.focus(), 0);
}

function validateForm() {
  let valid = true; elements.titleError.textContent = ""; elements.dateError.textContent = "";
  if (!elements.title.value.trim()) { elements.titleError.textContent = "Enter a title for this task."; valid = false; }
  if (!elements.dueDate.value) { elements.dateError.textContent = "Select a due date."; valid = false; }
  return valid;
}

function showToast(message) { elements.toast.textContent = message; elements.toast.classList.add("visible"); clearTimeout(showToast.timer); showToast.timer = setTimeout(() => elements.toast.classList.remove("visible"), 2200); }
function closeMenu() { elements.sidebar.classList.remove("open"); elements.overlay.classList.remove("visible"); elements.menu.setAttribute("aria-expanded", "false"); }

document.querySelectorAll("[data-filter]").forEach(button => button.addEventListener("click", () => {
  activeFilter = button.dataset.filter; document.querySelectorAll("[data-filter]").forEach(item => item.classList.toggle("active", item === button)); renderTasks();
}));
elements.search.addEventListener("input", renderTasks);
elements.taskList.addEventListener("click", event => {
  const action = event.target.closest("[data-action]")?.dataset.action; const row = event.target.closest("[data-id]"); if (!action || !row) return;
  const task = tasks.find(item => item.id === Number(row.dataset.id));
  if (action === "toggle") { task.completed = !task.completed; renderTasks(); showToast(task.completed ? "Task completed" : "Task moved to pending"); }
  if (action === "edit") openTaskDialog(task);
  if (action === "delete") { taskToDelete = task.id; elements.deleteDialog.showModal(); }
});

elements.form.addEventListener("submit", event => {
  event.preventDefault(); if (!validateForm()) return;
  const id = Number(elements.taskId.value); const task = { title: elements.title.value.trim(), description: elements.description.value.trim(), dueDate: elements.dueDate.value };
  if (id) Object.assign(tasks.find(item => item.id === id), task); else tasks.unshift({ id: Date.now(), completed: false, ...task });
  elements.dialog.close(); renderTasks(); showToast(id ? "Task updated" : "Task added");
});

document.querySelector("#addTaskButton").addEventListener("click", () => openTaskDialog());
document.querySelector("#emptyAddButton").addEventListener("click", () => openTaskDialog());
document.querySelector("#closeDialogButton").addEventListener("click", () => elements.dialog.close());
document.querySelector("#cancelDialogButton").addEventListener("click", () => elements.dialog.close());
document.querySelector("#cancelDeleteButton").addEventListener("click", () => elements.deleteDialog.close());
document.querySelector("#confirmDeleteButton").addEventListener("click", () => { tasks = tasks.filter(task => task.id !== taskToDelete); taskToDelete = null; elements.deleteDialog.close(); renderTasks(); showToast("Task deleted"); });
elements.menu.addEventListener("click", () => { const open = elements.sidebar.classList.toggle("open"); elements.overlay.classList.toggle("visible", open); elements.menu.setAttribute("aria-expanded", String(open)); });
elements.overlay.addEventListener("click", closeMenu);
document.querySelectorAll("[data-nav]").forEach(button => button.addEventListener("click", () => { document.querySelectorAll("[data-nav]").forEach(item => item.classList.toggle("active", item === button)); if (button.dataset.nav === "Completed") { activeFilter = "completed"; document.querySelector('[data-filter="completed"]').click(); } closeMenu(); }));

setTimeout(() => { elements.loading.hidden = true; renderTasks(); }, 500);
