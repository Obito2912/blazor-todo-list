# My Quest UI prototype

This dependency-free prototype lets the team review the shared dashboard structure while the official Blazor solution, data layer, services, and authentication are still being prepared.

## Run it

Open `index.html` directly in a browser, or serve this folder locally:

```powershell
cd ui-prototype
python -m http.server 8080
```

Then visit `http://localhost:8080`.

## Implemented interactions

- Responsive sidebar and mobile navigation
- Search by task title or description
- All, pending, and completed filters
- Add and edit task form with validation
- Mark tasks completed or pending
- Delete confirmation
- Loading, empty, and success states
- Sample data only; no database or authentication dependency

The CSS variables at the top of `styles.css` are design tokens. Once the group selects a design, its colors, radii, and shadows can be applied without changing the component structure.
