import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        { path: '/', redirect: { name: 'upstreams' } },
        {
            path: '/',
            component: () => import('@/components/layout/AppShell.vue'),
            children: [
                {
                    path: 'upstreams',
                    name: 'upstreams',
                    component: () => import('@/views/UpstreamsListView.vue'),
                    meta: { crumbs: 'Configure / Upstreams' },
                },
                {
                    path: 'upstreams/new',
                    name: 'upstream-create',
                    component: () => import('@/views/UpstreamCreateView.vue'),
                    meta: { crumbs: 'Configure / Upstreams / New' },
                },
                {
                    path: 'upstreams/:id/edit',
                    name: 'upstream-edit',
                    component: () => import('@/views/UpstreamEditView.vue'),
                    props: true,
                    meta: { crumbs: 'Configure / Upstreams / Edit' },
                },
            ],
        },
        { path: '/:pathMatch(.*)*', redirect: { name: 'upstreams' } },
    ],
})

export default router
